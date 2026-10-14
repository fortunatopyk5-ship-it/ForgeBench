# Open questions

1. Can YM-CUTOFF-BRIDGE-1 be written as a clean theorem about weak convergence of positive spectral measures on varying OS Hilbert spaces with no uniform correlator prefactor assumption?

2. Can the Shen–Zhu–Zhu covariance proof be abstracted to an explicit inequality
gamma_a >= c lambda_a/v_a
with all constants tracked and independent of volume?

3. At weak coupling, after gauge-invariant block integration, is there a scale-dependent functional inequality whose effective lambda_k/v_k remains controlled until the matching scale ell~Lambda_YM^{-1}?

4. Can Balaban's small-field RG estimates be combined with a boundary-condition-uniform mixing estimate without assuming the conclusion in Dobrushin form?

5. Which gauge-invariant local observable class is easiest for proving density in the reconstructed vacuum sector while retaining manageable renormalization?

6. What exact lower bound on gamma(beta_W) is needed after including the two-loop logarithmic power in a(beta_W) Lambda_YM?

7. Can reflection positivity be kept at the original lattice level while RG is used only to prove correlation estimates, avoiding any need for RP of the blocked effective action?

8. Is there a useful transfer-matrix comparison inequality (rather than stochastic LSI) that is stable under integrating UV modes and yields gamma(a)>=c a Lambda_YM?


## YM-CROSSOVER-1 focused questions — Iteration 2

9. YM-MATCH-EXTRACT-1: Can the Balaban 4D effective-density estimates at a fixed small matching coupling be translated into the global norm of crossover_spec.md with constants independent of UV cutoff and physical volume?

10. Which global coordinate beta=ell_W(Phi) is best adapted both to the perturbative matching chart and to the product-Haar strong-coupling chart? A proof of quantitative equivalence between these coordinates is needed.

11. Can one derive a rigorous one-step enclosure
beta' <= beta-delta(beta,r),
r' <= A(beta)r+B(beta)+C(beta)r^2
on a compact intermediate-coupling tube, with delta bounded uniformly away from zero?

12. Can the final super-polymer activity norm
Q_{alpha,mu}(z_Phi)
be bounded directly by beta and r, producing an explicit beta_sc,R threshold for D_KP?

13. Source issue: what is the weakest source-dependent RG estimate sufficient to bound the conditional-covariance term from eliminated shells? Is a random-walk/locality estimate for fluctuation propagators plus polymer source bounds enough?

14. Can the source transport theorem be made observable-independent for the algebra of bounded local gauge-invariant cylindrical functions, so a common exponent is available on a dense physical sector?

15. For validated numerics, which finite set of analytically bounded RG coefficients can be enclosed rigorously enough that verify_crossover_tube.py checks the remaining induction?


## Iteration 3 focused questions

16. What exact rooted counting theorem converts CMP119/CMP122 native d_j(X)-decay into a volume-uniform anchored activity norm? Track the entropy constant explicitly.

17. Can the CMP119 restricted analytic domains U_j^c(X,alpha_0,alpha_1) be organized into a complete domain-indexed Banach family on which one RT/R step is a bounded map?

18. What is the exact normalization relating the classical coefficient 1/g_j^2 to a chosen one-plaquette/Wilson coordinate? Is ell_W the right coordinate at all, or should the crossover coordinate be defined directly by the extracted action coefficient?

19. Can Balaban localization/extraction be isolated as a bounded linear operator after sector labels are frozen, or is localization intrinsically part of a nonlinear cluster-expansion chart transition?

20. Which theorem gives derivative/Cauchy bounds for R^(j), B^(j), C_k^(n), R'^(k) from their analytic-extension domains, with constants uniform in scale and domain history?


## Iteration 4 exact blockers before K_0

21. What is the exact primary definition of an admissible CMP119 history Sigma=(Omega_j,Lambda_j,S_j,...) and all nesting/separation clauses?

22. For a fixed field configuration, are complete histories pairwise disjoint, uniformly finite-overlap, or genuinely summed with nontrivial weights?

23. What source majorant controls the T_Sigma operation and the internal S_j sum? Can it provide a cutoff-uniform H_hist?

24. Prove in the exact pi_j cube geometry that |X|<=a_0+a_1 d_j(X) with scale-independent a_0,a_1. The remaining fixed-history animal count is then explicit.

25. Is there a source theorem controlling pairs (Sigma,X), or must history weights and polymer weights remain separate?

26. Construct normalized analytic charts Psi_{Sigma,j,X}:U_ref->U^*_{Sigma,j}(X) with cutoff-uniform inverse/distortion and delta_ref>0.

27. What norm controls the localized coupling-profile defect q(x)=1/g^2(x)-c uniformly over histories?

28. In the exact Balaban inductive class, which compatibility constraints are genuine cross-history identities and which are only encoded by admissibility of the index set?

29. Prove continuity of Rec_k in a history-weighted topology and use it to close the actual-density graph M_adm,k.

30. Upgrade native RT/R class preservation to R(M_adm,k) subset M_adm,k+1 in the same fixed reference topology.


## Iteration 5 — resolved history questions and remaining functional-analytic blockers

Resolved/source-verified:
- Q21 core history geometry: Omega/Lambda interleaving, S_j structure, and recursive generation are now direct-primary verified. A nonrecursive finite characterization after all R modifications is still unnecessary/unavailable; use the generative definition.
- Q22: raw P/Q/R characteristic decompositions are exact hard partitions, but compressed Eq. (2.18) histories are not indexed by a complete chi_Sigma. Raw choices are resummed into zeta/T.
- Q23 source mechanism: admissible-sequence entropy is explicitly bounded before CMP122-II (1.79) and paid by large-field p_0(g), p_0(g)^2/tree-size penalties; local T operator bounds (1.75),(1.89) are direct-primary verified.
- Q27 correction: localized coupling profile is derived after fixing the phi_Lambda cutoff convention; it is not an independent physical coordinate.

31. Choose the correct volume-independent density target D. A global sup norm of the unnormalized rho_k is unsuitable because UV-stability bounds carry extensive exp(E|T|) factors. Should D be a relative density, log-density/local interaction, or projective local-density norm?

32. Convert CMP122-II Eq. (1.79) and induction (1.80)-(1.88) into a fixed compressed-history kernel bound
||K_H||<=tau_H
with H_0=sum_H tau_H finite uniformly in cutoff, volume and k.

33. Choose source-faithful weights w_H>=1 such that H_1=sum_H tau_H/w_H is uniformly finite and compatible with the normalized analytic reference charts.

34. Prove that the fixed-reference-chart native RG extends from the physical source class to an open U subset B_amb and is C^1/C^2 there.

35. Decide whether the deterministic cutoff assignment Lambda->phi_Lambda can be chosen Euclidean-covariantly and compatibly with restriction/RG transitions. This is the exact condition needed to make the derived coupling profile canonical.

36. RESOLVED in Iteration 5: direct-primary CMP109 gives the shortest-tree metric; YM-BALABAN-TREE-COERCIVITY-1 proves |X|<=16(d_j(X)+1) in d=4 and YM-BALABAN-TREE-SHELL-COUNT-1 gives an explicit scale/volume-uniform fixed-history shell count. This does not count histories.


## Iteration 6 — YM-HISTORY-NORM-1

Resolved:
- Global complete-history H_0=sum_H tau_H is rejected as a necessary target (YM-GLOBAL-H0-FAIL-1).
- Preferred D is connected relative log-density / activity space with anchored norm.
- CMP109 (0.26) supplies direct source geometric summability for exponential tree-decay activities.
- Local H_0 is proved for the improved connected T-history components and regular post-R connected activities.
- Tree normalization has no missing M factor.

37. Can the ENTIRE compressed CMP119 Eq. (2.18) outer Omega/Lambda sum, including T/zeta data not already absorbed by the local R-operation, be represented as a bounded operator-polymer family in the same connected local activity topology?

38. Prove a cutoff/volume/RG-depth-uniform local Lipschitz estimate h_1 for the map
pre-resummed compressed history data -> connected local activities.
This is now the central continuity blocker.

39. Can the CMP116/CMP122 cluster-expansion extraction/localization maps be organized as analytic maps between the normalized fixed-reference activity Banach spaces, thereby supplying the required h_1 by Cauchy/Frechet estimates?

40. Choose the exact treatment of field-independent vacuum terms: quotient by additive constants versus a separate vacuum-energy-density coordinate. The choice must be compatible with source ratios and the RG map.

41. Instantiate the abstract source-ratio cancellation theorem for Balaban marked local sources: prove that source-independent connected clusters cancel in W(J_A)=log Z(J_A)-log Z(0), with anchored bounds uniform in volume.

42. For the intermediate relative metric d_{k,Y} in CMP122-II (1.97)-(1.99), either prove an anchored relative-metric summability theorem or show rigorously that passage into the final B-sector is sufficient before entering D_rel.


## Iteration 7 — C^1 history-resummation blockers

43. Extract a source-exact support/collar theorem for every history-resummation submap:
M_{gamma eta}=0 unless eta lies in a fixed enlargement of gamma (or a stronger connected-union relation). Record the enlargement in block units at each scale.

44. RESOLVED for the regular R' coefficient sector in Iteration 8: do not identify a hidden source threshold with a Banach radius. Instead use the explicit sufficient KP radius
r_*=a exp[-16a-2theta],
then choose gamma_R' so that
r=C_geo(lambda_in)e^{-p_0(gamma_R')}<r_*
and rho=(r+r_*)/2.

45. RESOLVED for the regular R' coefficient sector in Iteration 8: generalized KP provides a volume/scale-uniform majorant for every Banach-algebra-valued activity family in the explicit anchored ball, so the canonical cluster logarithm is normally convergent and Banach-holomorphic.

46. Build the scale-graded activity coefficient Banach space explicitly using A_{j,X}=H^infinity(U_j^c(X,...)) and prove every source restriction/localization map used by history resummation is bounded between the relevant coordinates.

47. Independently, extract an inner/outer normalized field/source analytic-domain pair with delta_ref>0 to justify Cauchy derivatives with respect to J/U. Different log powers in alpha_0,j and alpha_1,j must be handled, not hidden by a single scalar rescaling.

48. RESOLVED/PARTIAL split in Iteration 8: for the regular R' canonical exact-union cluster/log chart, D_eta H_gamma=0 unless eta subset gamma and r_col=0. Bounded C^1 is closed. The off-diagonal Schur estimate is proved under the explicit additional reserve theta-mu_out-nu in the CMP109 summability regime; verifying that extra reserve for a concrete numerical Balaban parameter choice remains open.

49. For T, identify the precise dependency collar X^+ and prove |X^+|<=C_col(1+d_k(X)) in the source geometry, completing the conditional hypothesis of YM-T-SECTOR-C1-1.

50. Decide whether the full compressed outer-history -> connected-activity map should be differentiated coefficientwise on the graded space or only after one further source resummation that removes hard characteristic boundaries.


## Iteration 8 — after regular R' C^1 closure

51. Upstream R' input construction: prove bounded C^1/locality for the map that constructs the pre-exponentiation F(X') of CMP122-II (1.91) from the preceding localized T/E/R/B/history data. This is now the nearest history-resummation blocker but was out of scope in Iteration 8.

52. Boundary R' / B' lane: instantiate an anchored Banach norm for the relative metric d_{k,Y} and prove the corresponding coefficient C^1 theorem for outputs intersecting the large-field boundary set.

53. Quasi-local regular R' Jacobian: extract enough source decay-budget information to verify existence of mu_out,nu>0 with theta-mu_out-nu in the CMP109 summability regime, or keep the Schur estimate explicitly conditional. Do not invent a numerical kappa threshold.

54. Source insertion interface: choose J_A and prove a bounded jmath_A:J_A->B_in before using physical sources as activity tangents.

55. Synthesis: define S_A:D_A^(mu)->F_A and prove any desired boundedness separately. Act_A is only a direct-sum activity vector.
