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
